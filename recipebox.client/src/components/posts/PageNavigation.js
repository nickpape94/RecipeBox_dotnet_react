import React, { Fragment, useState } from 'react';
import PropTypes from 'prop-types';

const PageNavigation = ({ pagination: { currentPage, totalItems, totalPages }, pageNumber, setPageNumber }) => {
	const numPages = [];
	for (let i = 1; i <= totalPages; i++) {
		numPages.push(i);
	}

	return (
		<Fragment>
			{totalItems > 9 && (
				<div className='pagination'>
					<ul>
						{currentPage !== 1 && (
							<li onClick={() => setPageNumber(1)}>
								<i className='fas fa-angle-double-left' />
							</li>
						)}
						{currentPage !== 1 && (
							<li onClick={() => setPageNumber(pageNumber - 1)}>
								<i className='fas fa-angle-left' />
							</li>
						)}

						{numPages.splice(currentPage - 1, 5).map(
							(value, key) =>
								key === 0 ? (
									<li className='active' key={key}>
										{value}
									</li>
								) : (
									<li onClick={() => setPageNumber(value)} key={key}>
										{/* history.replace(`/posts/pageNumber/1`); */}
										{value}
									</li>
								)
						)}
						{totalPages !== pageNumber && (
							<li onClick={() => setPageNumber(pageNumber + 1)}>
								<i className='fas fa-angle-right' />
							</li>
						)}
						{totalPages !== pageNumber && (
							<li onClick={() => setPageNumber(totalPages)}>
								<i className='fas fa-angle-double-right' />
							</li>
						)}
					</ul>
				</div>
			)}
		</Fragment>
	);
};

export default PageNavigation;
